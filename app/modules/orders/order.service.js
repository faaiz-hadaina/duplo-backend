const axios = require("axios");
const Transaction = require("../transaction/transaction.model");
const calculateCreditScore = require("../../utils/calculateScore");

class OrderService {
  static async processOrder(orderData) {
    const transaction = new Transaction(orderData);
    const savedOrder = await transaction.save();
    const taxPayload = {
      order_id: savedOrder._id,
      platform_code: "022",
      order_amount: savedOrder.amount,
    };

    const taxApiResponse = await axios.post(process.env.TAX_URL, taxPayload);

    return [savedOrder, taxApiResponse];
  }

  static async calculateCreditScore(businessID) {
    const transactions = await Transaction.find({ businessID });
    const totalAmount = transactions.reduce(
      (acc, transaction) => acc + transaction.amount,
      0
    );
    const totalOrders = transactions.length;

    return calculateCreditScore(totalAmount, totalOrders);
  }
}

module.exports = OrderService;
