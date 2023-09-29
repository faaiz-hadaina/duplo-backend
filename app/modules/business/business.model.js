const mongoose = require('mongoose');
const mongoosePaginate = require('mongoose-paginate-v2');

const BusinessSchema = new mongoose.Schema({
  name: String,
  password: String,
  businessID: Number,
  totalAmount: Number
});

BusinessSchema.plugin(mongoosePaginate);
module.exports = mongoose.model('Business', BusinessSchema);
