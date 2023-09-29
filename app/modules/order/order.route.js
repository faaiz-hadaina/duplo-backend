const express = require('express');
const ordersController = require('./order.controller');
const departmentAuth = require('../auth/department/department.service')

const router = express.Router();

router.post('/api/orders', departmentAuth, ordersController.processOrder);
router.get('/api/order-details/:businessID', ordersController.getOrderDetails);

module.exports = router;
