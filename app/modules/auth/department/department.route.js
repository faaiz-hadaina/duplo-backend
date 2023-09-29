const router = require('express').Router()
const authController = require('./department.controller')
const businessAuth = require('../../auth/business/business.service')

router.post('/api/dept/register', businessAuth, authController.register)
router.post('/api/dept/login', authController.login)

module.exports = router;