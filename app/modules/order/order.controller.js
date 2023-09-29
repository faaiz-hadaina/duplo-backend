const createLogger = require("../../service/winston.service");
const OrderService = require("./order.service");
const Transaction = require('../transaction/transaction.model');
const Business = require('../business/business.model')

const logger = createLogger("controller", "controller.log");

class APIFeatures {
  constructor(query, queryString){
      this.query = query;
      this.queryString = queryString;
  }

  paginating(){
      const page = this.queryString.page * 1 || 1
      const limit = this.queryString.limit * 1 || 9
      const skip = (page - 1) * limit
      this.query = this.query.skip(skip).limit(limit)
      return this;
  }
}

const orderCtrl = {
  // Task 1: Curate orders from each business’s department lead
  processOrder: async (req, res) => {
    try {
      // Get the businessID from req.user
      const businessID = req.department.businessID;

      // Create the orderData object
      const orderData = {
        businessID, // Set the businessID
        amount: req.body.amount,
        date: req.body.date,
        status: req.body.status,
      };

      // Log the transaction data into MongoDB and send it to the tax authority
      const [savedOrder, taxApiResponse] = await OrderService.processOrder(orderData);

      logger.info('Tax API response:', taxApiResponse.data);
      logger.info('Order processed successfully');

      await Business.findOneAndUpdate(
        { businessID },
        { $inc: { totalAmount: orderData.amount } }
      );
      

      res.status(200).json({ message: 'Order processed successfully', savedOrder });
    } catch (error) {
      logger.error('Error processing order:', error);
      res.status(500).json({ message: 'Internal Server Error' });
    }
  },
  

  // Provide an API endpoint for a business to get their credit score
  getCreditScore: async (req, res) => {
    try {
      const businessID = req.business.businessID;

      // Get the credit score from the OrderService
      const creditScore = await OrderService.calculateCreditScore(businessID);

      res.status(200).json({ creditScore });
    } catch (error) {
      logger.error("Error fetching credit score:", error);
      res.status(500).json({ message: "Internal Server Error" });
    }
  },

  getOrderDetails: async (req, res) => {
    const businessID = Number(req.params.businessID);
    const { page, limit } = req.query; // Get page and limit from query params
  
    try {
      const today = new Date();
      const startOfDay = new Date(today);
      startOfDay.setHours(0, 0, 0, 0);
  
      // Use aggregation pipeline to calculate order details
      let aggregationPipeline = [
        {
          $match: {
            businessID: businessID,
            status: 'out-of-stock',
          },
        },
        {
          $group: {
            _id: null,
            totalOrders: { $sum: 1 },
            totalAmount: { $sum: "$amount" },
            todayTotalOrders: {
              $sum: {
                $cond: [
                  {
                    $and: [
                      { $eq: ["$businessID", businessID] },
                      { $gte: ["$date", startOfDay] },
                    ],
                  },
                  1,
                  0,
                ],
              },
            },
            todayTotalAmount: {
              $sum: {
                $cond: [
                  {
                    $and: [
                      { $eq: ["$businessID", businessID] },
                      { $gte: ["$date", startOfDay] },
                    ],
                  },
                  "$amount",
                  0,
                ],
              },
            },
          },
        },
      ];
  
      // Apply pagination if page and limit are provided
      if (page && limit) {
        const features = new APIFeatures(Transaction.aggregate(aggregationPipeline), req.query)
          .paginating();
        aggregationPipeline = features.query; // Update the aggregation pipeline
      }
  
      const orderDetails = await aggregationPipeline.exec();
  
      if (orderDetails.length === 0) {
        return res.status(404).json({ error: "No orders found" });
      }
  
      const result = orderDetails[0];
  
      res.status(200).json({
        totalOrders: result.totalOrders,
        totalAmount: result.totalAmount,
        todayTotalOrders: result.todayTotalOrders,
        todayTotalAmount: result.todayTotalAmount,
      });
    } catch (error) {
      console.error(error);
      res.status(500).json({ error: "Internal Server Error" });
    }
  },  
};

module.exports = orderCtrl;
