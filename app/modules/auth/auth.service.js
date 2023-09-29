const jwt = require("jsonwebtoken");
const Department = require("../department/department.model");

const auth = async (req, res, next) => {
  try {
    let token = req.headers["authorization"].split(" ")[1];
    let decoded = jwt.verify(token, process.env.SECRET);
    const user = await Department.findById(decoded.id);

    if (!user) {
      return res.status(401).json({ message: "User not found." });
    }

    req.user = user;
    next();
  } catch (err) {
    console.log(err);
    res.status(401).json({
      message: "Couldn't Authenticate",
    });
  }
};

module.exports = auth;
