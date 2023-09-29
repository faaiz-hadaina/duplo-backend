const mongoose = require('mongoose');
const mongoosePaginate = require('mongoose-paginate-v2');

const transactionSchema = new mongoose.Schema({
  businessID: Number,
  amount: Number,
  date: Date,
  status: String,
});

transactionSchema.plugin(mongoosePaginate);
module.exports = mongoose.model('Transaction', transactionSchema);
