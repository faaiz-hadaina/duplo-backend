const Department = require("../modules/department/department.model");
const generateUniqueBusinessID = async () => {
  while (true) {
    const businessID = Math.floor(Math.random() * 9000) + 1000;
    const existingUser = await Department.findOne({ businessID });
    if (!existingUser) {
      return businessID;
    }
  }
};

module.exports = generateUniqueBusinessID;
