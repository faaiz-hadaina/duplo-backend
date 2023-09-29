const Department = require('../modules/department/department.model');
// Function to generate a unique random businessID
const generateUniqueBusinessID = async () => {
    while (true) {
      const businessID = Math.floor(Math.random() * 9000) + 1000; // Generate a random number between 1000 and 9999
      const existingUser = await Department.findOne({ businessID });
      if (!existingUser) {
        return businessID; // Return the generated businessID if it's unique
      }
    }
  };

  module.exports = generateUniqueBusinessID