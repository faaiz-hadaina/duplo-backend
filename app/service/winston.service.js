const winston = require('winston');
const path = require('path');
const { format } = require('winston');
const util = require('util');

function createLogger(service, filename) {
  const logFormat = format.combine(
    format.timestamp({
      format: 'YYYY-MM-DD HH:mm:ss',
    }),
    format.errors({ stack: true }),
    format.splat(),
    format.json(),
    format.printf(({ timestamp, level, message, ...metadata }) => {
      // Use util.inspect to handle circular structures
      let metaString = util.inspect(metadata, { depth: null });
      if (metaString !== '{}') {
        metaString = ` ${metaString}`;
      }
      return `${timestamp} ${level}: ${message}${metaString}`;
    }),
  );

  return winston.createLogger({
    level: process.env.LOG_LEVEL || 'info',
    format: logFormat,
    defaultMeta: { service },
    transports: [
      new winston.transports.File({
        filename: path.join('logs', filename),
        level: 'error',
        maxsize: 5 * 1024 * 1024, // 5MB
        maxFiles: 5,
        tailable: true,
      }),
      new winston.transports.File({
        filename: path.join('logs', filename),
        level: 'info',
        maxsize: 5 * 1024 * 1024,
        maxFiles: 5,
        tailable: true,
      }),
      new winston.transports.File({
        filename: path.join('logs', filename),
        level: 'warn',
        maxsize: 5 * 1024 * 1024,
        maxFiles: 5,
        tailable: true,
      }),
      new winston.transports.Console({
        format: format.combine(format.colorize(), logFormat),
      }),
    ],
  });
}

module.exports = createLogger;
