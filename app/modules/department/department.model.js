const mongoose = require('mongoose');

const DepartmentSchema = new mongoose.Schema({
  name: String,
  password: String,
  businessID: Number,
});

module.exports = mongoose.model('Department', DepartmentSchema);
