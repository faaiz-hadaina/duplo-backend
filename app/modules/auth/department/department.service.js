const jwt = require('jsonwebtoken');
const Department = require('../../department/department.model');

const departmentAuth = async (req, res, next) => {
    try {
        let token = req.headers["authorization"].split(" ")[1];
        let decoded = jwt.verify(token, process.env.SECRET);
        const department = await Department.findById(decoded.id)

        if (!department) {
            return res.status(401).json({ message: 'User not found.' });
        }

        req.department = department;
        next();
    } catch (err) {
        console.log(err);
        res.status(401).json({
            message: "Unauthorized"
        });
    }
}

module.exports = departmentAuth;
