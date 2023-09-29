const express = require('express');
const morgan = require('morgan');
const cors = require('cors');
const mongoose = require('mongoose');
const createLogger = require('./app/service/winston.service');
const routes = require("./app/routes/index");
const rateLimit = require('express-rate-limit');

const logger = createLogger('index', 'index.log');
const app = express();
const PORT = process.env.PORT || 3000;

// Load environment variables from a .env file
require('dotenv').config();

// MongoDB connection logic
const connectToMongoDB = async () => {
  try {
    const URI = process.env.MONGODB_URL;
    await mongoose.connect(URI, {
      useNewUrlParser: true,
      useUnifiedTopology: true,
    });
    logger.info('Connected to MongoDB');
  } catch (error) {
    logger.error('Error connecting to MongoDB:', error);
    console.error('Error connecting to MongoDB:', error);
    throw error;
  }
};

connectToMongoDB();

const corsOptions = {
  origin: '*',
};

const limiter = rateLimit({
  windowMs: 10 * 60 * 10000,
  max: 100
})

app.use(express.json());
app.use(morgan('combined'));
app.use(cors(corsOptions));
app.use(limiter)

app.use("/", routes);

app.get('/', (req, res) => {
  res.send('Welcome');
});

app.use((err, req, res, next) => {
  logger.error(err.stack);
  res.status(500).json({ message: 'Internal Server Error' });
});

// Start the server
app.listen(PORT, () => {
  logger.info(`Server is running on port ${PORT}`);
  console.log(`Server is running on port ${PORT}`);
});

module.exports = app;
