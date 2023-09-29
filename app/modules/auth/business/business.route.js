const router = require('express').Router()
const businessController = require('./business.controller')
const ordersController = require('../../order/order.controller');
const businessAuth = require('./business.service')

router.post('/api/business/register', businessController.register)
router.post('/api/business/login', businessController.login)
router.get('/api/credit-score', businessAuth, ordersController.getCreditScore);
router.get('/api/getAll', businessController.getAllBusinesses)

module.exports = router;