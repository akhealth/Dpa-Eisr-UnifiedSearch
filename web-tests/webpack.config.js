const path = require('path')

// We use webpack in the web tests project mainly so we can use the
// same syntax to import our components and other scripts that we use
// in the web project itself, e.g., `import Search from '-/pages/Search/Search'`.
module.exports = {
  resolve: {
    alias: {
      '-': path.join(__dirname, '../web/client')
    }
  }
}
