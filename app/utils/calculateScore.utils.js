function calculateCreditScore(totalAmount, totalOrders) {
  return totalAmount / (totalOrders * 100);
}

module.exports = calculateCreditScore;
