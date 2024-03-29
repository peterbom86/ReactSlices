var path = require('path')
var webpack = require('webpack')

module.exports = {
  devtool: 'source-map',
  entry: [
    'webpack-hot-middleware/client',
    './index'
  ],
  output: {
    path: path.join(__dirname, 'dist'),
    filename: 'bundle.js',
    publicPath: '/static/'
  },
  plugins: [
    new webpack.optimize.OccurenceOrderPlugin(),
    new webpack.HotModuleReplacementPlugin()
  ],
  module: {
    loaders: [
      {
        test: /\.js$/,
        loaders: ['babel'],
        exclude: /node_modules/,
        include: __dirname
      },
      {
        test: /\.jsx$/,
        loaders: ['babel'],
        exclude: /node_modules/,
        include: __dirname
      },
      {
        test: /\.json$/, loader: 'json'
      }
    ]
  },
  resolve: {
    extensions: ['', '.js', '.jsx', '.json']
  }
}
