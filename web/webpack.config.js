const path = require('path')
const webpack = require('webpack')
const UglifyJsPlugin = require('uglifyjs-webpack-plugin')
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin')
const HtmlWebpackPlugin = require('html-webpack-plugin')
const CleanWebpackPlugin = require('clean-webpack-plugin')
const BrowserSyncPlugin = require('browser-sync-webpack-plugin')
const CopyWebpackPlugin = require('copy-webpack-plugin')
const ExtractTextPlugin = require('extract-text-webpack-plugin')

// File/directory helper to get absolute path
const resolve = d => path.join(__dirname, d)

// Determine if this is a production build
const isProduction = process.env.NODE_ENV === 'production'

// Determine the correct search API URL to inject for client-side code
const API_URL = process.env.SEARCH_API_URL

// Determine the correct search web URL for BrowserSync
const WEB_URL = process.env.SEARCH_WEB_URL

// Determine the correct path to serve static assets from
const PUBLIC_PATH = process.env.SEARCH_WEB_URL

// Extract the CSS into a separate bundle
const extractSass = new ExtractTextPlugin(
  isProduction ? '[name].[hash].css' : '[name].css'
)

// Default plugins
const plugins = [
  // Clean the output directory before each build
  new CleanWebpackPlugin([ 'wwwroot' ]),

  // Extract the CSS into a separate bundle
  extractSass,

  // Remove locales from momentjs - most are unneeded and can be imported directly
  new webpack.IgnorePlugin(/^\.\/locale$/, /moment$/),

  // Define globals for client-side scripts
  new webpack.DefinePlugin({
    API_URL: JSON.stringify(API_URL),
    ENVIRONMENT: JSON.stringify(process.env.NODE_ENV || 'development')
  }),

  // Inject client-side assets into the site html template
  new HtmlWebpackPlugin({
    template: 'client/index.html'
  }),

  // Use BrowserSync to watch and reload file changes
  new BrowserSyncPlugin(
    {
      host: 'localhost',
      port: 5002,
      proxy: WEB_URL,
      files: [ isProduction ? 'wwwroot/*' : 'wwwroot/**/*.!(html)' ],
      ghostMode: {
        // Turn off form submit event syncing
        forms: {
          submit: false
        }
      },
      // Don't automatically open a browser window/tab
      open: false
    },
    {
      reload: false
    }
  ),

  // Copy necessary files to output directory
  new CopyWebpackPlugin([
    {
      from: 'client/favicon.ico',
      to: 'favicon.ico'
    }
  ])
]

const config = {
  mode: isProduction ? 'production' : 'development',

  entry: {
    app: resolve('client/index.js')
  },

  output: {
    filename: isProduction ? '[name].[hash].js' : '[name].js',
    publicPath: PUBLIC_PATH,
    path: resolve('wwwroot')
  },

  resolve: {
    alias: {
      '-': resolve('client'),
      styles: resolve('client/styles')
    }
  },

  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /node_modules/,
        loaders: [ 'babel-loader' ]
      },
      {
        test: /\.scss$/,
        use: extractSass.extract([
          'css-loader',
          {
            loader: 'sass-loader',
            options: {
              includePaths: [
                resolve('node_modules/purecss-sass/vendor/assets/stylesheets')
              ]
            }
          }
        ])
      },
      {
        test: /\.(jpe?g|png|gif|svg)$/i,
        loaders: [
          {
            loader: 'file-loader',
            options: {
              name: isProduction ? '[name].[hash].[ext]' : '[name].[ext]',
              publicPath: `${PUBLIC_PATH}/images/`,
              outputPath: 'images/'
            }
          }
        ]
      }
    ]
  },

  optimization: {
    splitChunks: {
      cacheGroups: {
        vendors: {
          test: /node_modules/,
          name: 'common',
          chunks: 'all'
        }
      }
    },
    minimizer: [
      new UglifyJsPlugin({
        cache: true,
        parallel: 4,
        sourceMap: true
      }),
      new OptimizeCssAssetsPlugin({
        cssProcessorOptions: {
          discardComments: {
            removeAll: true
          }
        }
      })
    ]
  },

  plugins,

  stats: {
    children: false,
    colors: true,
    env: true
  },

  watchOptions: {
    aggregateTimeout: 300,
    poll: 500
  }
}

module.exports = config
