const bcrypt = require('bcrypt');
const jwt = require('jsonwebtoken')
const Business = require('../../business/business.model');
const createLogger = require("../../../service/winston.service");
const generateUniqueBusinessID = require('../../../utils/generateRandom.utils')

const logger = createLogger("auth", "auth.log");

class APIfeatures {
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

const businessAuthCtrl = {

    register: async (req, res) => {
        try {
          const { name, password } = req.body;
          const salt = await bcrypt.genSalt(10);
          const encryptedPassword = await bcrypt.hash(password, salt);
    
          const businessName = await Business.findOne({ name });
          if (businessName) {
            return res.status(400).json({
              message: `${name} is already registered`,
              success: false,
            });
          }
    
          if (password.length < 6) {
            return res.status(400).json({
              msg: 'Password must be at least 6 characters.',
              success: false,
            });
          }
    
          const businessID = await generateUniqueBusinessID();
    
          const newAccount = new Business({
            name,
            password: encryptedPassword,
            businessID, // Set the generated businessID
          });
    
          await newAccount.save();
    
          res.status(201).json({
            msg: 'Registration Success!',
            business: {
              ...newAccount._doc,
              password: '',
            },
            success: true,
          });
        } catch (err) {
          logger.error(`Registration failed: ${err.message}`);
          res.status(500).json({
            message: err.message,
            success: false,
          });
        }
      },

    login: async (req, res) => {
        try {
            const { name, password} = req.body;
            const business = await Business.findOne({name});
            if(!business) res.status(400).json({
                message: `${business} does not exist`,
                success: false
            })
            const isMatch = await bcrypt.compare(password, business.password);
            if(isMatch) {
                const token = jwt.sign({
                    id: business._id,
                    name: business.name
                }, process.env.SECRET)
                res.status(200).json({ token: token });
            } else {
                return res.status(404).json({
                    message: 'Invalid Credentials',
                    success: false
                })
            }
        } catch (err) {
            logger.error(`Login failed: ${err.message}`);
            return res.status(500).json({
                message: err.message,
                success: false
            })
        }
    },
    getAllBusinesses: async (req, res) => {
      try {
        const { page, limit } = req.query;
    
        let query = Business.find();
    
        // Apply pagination if page and limit are provided
        if (page && limit) {
          const features = new APIfeatures(query, req.query).paginating();
          query = features.query; // Update the query
        }
    
        const businesses = await query.exec();
    
        res.status(200).json(businesses);
      } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Internal Server Error' });
      }
    },
    
}

module.exports = businessAuthCtrl