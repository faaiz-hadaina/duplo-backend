# Use an official Node.js runtime as the base image
FROM node:18

# Set the working directory in the container
WORKDIR /index

# Copy package.json and package-lock.json to the container
COPY package*.json ./

# Install project dependencies
RUN npm install

# Copy the rest of the application code to the container
COPY . .

# Expose the port that your Node.js app will run on
EXPOSE 3000

# Start your Node.js application
CMD [ "node", "index.js" ]
