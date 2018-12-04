# # base image
# FROM node:9.6.1

# # set working directory
# RUN mkdir /usr/src/app
# WORKDIR /usr/src/app

# # add `/usr/src/app/node_modules/.bin` to $PATH
# ENV PATH /usr/src/app/node_modules/.bin:$PATH

# # install and cache app dependencies
# COPY package.json /usr/src/app/package.json
# RUN npm install
# RUN npm install react-scripts@1.1.1 -g

# # Expose PORT 3000 on our virtual machine so we can run our server
# EXPOSE 3000

# ENTRYPOINT ["npm", "start"]

# FROM node:latest

# EXPOSE 3000
# WORKDIR /bindmount
# COPY package-lock.json package.json ./
# RUN npm install --no-progress --ignore-optional

# CMD npm start     # webpack-dev-server --host 0.0.0.0 --hot

FROM node:8

ADD package.json /package.json

ENV NODE_PATH=/node_modules
ENV PATH=$PATH:/node_modules/.bin
RUN npm install

WORKDIR /app
ADD . /app

EXPOSE 3000
EXPOSE 35729

ENTRYPOINT ["/bin/bash", "/app/run.sh"]
CMD ["start"]