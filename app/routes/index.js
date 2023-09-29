const { Router } = require("express");
const router = Router();

const ordersRoutes = require('../modules/order/order.route');
const departmentAuthRoutes = require('../modules/auth/department/department.route');
const businessAuthRoutes = require('../modules/auth/business/business.route')

router.use('/', ordersRoutes);
router.use('/', departmentAuthRoutes)
router.use('/', businessAuthRoutes)

module.exports = router;