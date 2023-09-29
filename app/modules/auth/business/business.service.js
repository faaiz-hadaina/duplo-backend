const jwt = require("jsonwebtoken");
const Business = require("../../business/business.model");

const businessAuth = async (req, res, next) => {
  try {
    let token = req.headers["authorization"].split(" ")[1];
    let decoded = jwt.verify(token, process.env.SECRET);
    const business = await Business.findById(decoded.id);

    if (!business) {
      return res.status(401).json({ message: "Business not found." });
    }

    req.business = business;
    next();
  } catch (err) {
    console.log(err);
    res.status(401).json({
      message: "Couldn't Authenticate",
    });
  }
};

module.exports = businessAuth;
