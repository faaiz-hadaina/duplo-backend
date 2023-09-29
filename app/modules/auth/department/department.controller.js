const bcrypt = require("bcrypt");
const jwt = require("jsonwebtoken");
const Department = require("../../department/department.model");
const createLogger = require("../../../service/winston.service");

const logger = createLogger("auth", "auth.log");

const authCtrl = {
  register: async (req, res) => {
    try {
      const { name, password } = req.body;
      const salt = await bcrypt.genSalt(10);
      const encryptedPassword = await bcrypt.hash(password, salt);

      const departmentName = await Department.findOne({ name });
      if (departmentName) {
        return res.status(400).json({
          message: `${name} is already registered`,
          success: false,
        });
      }

      if (password.length < 6) {
        return res.status(400).json({
          msg: "Password must be at least 6 characters.",
          success: false,
        });
      }

      const businessID = req.business.businessID;

      const newAccount = new Department({
        name,
        password: encryptedPassword,
        businessID,
      });

      await newAccount.save();

      res.status(201).json({
        msg: "Registration Success!",
        department: {
          ...newAccount._doc,
          password: "",
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
      const { name, password } = req.body;
      const department = await Department.findOne({ name });
      if (!department)
        res.status(400).json({
          message: `${department} does not exist`,
          success: false,
        });
      const isMatch = await bcrypt.compare(password, department.password);
      if (isMatch) {
        const token = jwt.sign(
          {
            id: department._id,
            name: department.name,
          },
          process.env.SECRET
        );
        res.status(200).json({ token: token });
      } else {
        return res.status(404).json({
          message: "Invalid Credentials",
          success: false,
        });
      }
    } catch (err) {
      logger.error(`Login failed: ${err.message}`);
      return res.status(500).json({
        message: err.message,
        success: false,
      });
    }
  },
};

module.exports = authCtrl;
