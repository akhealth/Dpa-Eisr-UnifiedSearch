import { JSDOM } from 'jsdom'
import Adapter from 'enzyme-adapter-react-16'
import Enzyme from 'enzyme'

const jsdom = new JSDOM('<!doctype html><html><body></body></html>')
const { window } = jsdom

// Make `window` properties available in the global scope in unit tests,
// so tests can depend on these properties as if they are running in
// a regular web browser. For more info, see
// https://github.com/airbnb/enzyme/blob/master/docs/guides/jsdom.md
const copyProps = (src, target) => {
  const props = Object.getOwnPropertyNames(src)
    .filter(prop => typeof target[prop] === 'undefined')
    .reduce(
      (result, prop) => ({
        ...result,
        [prop]: Object.getOwnPropertyDescriptor(src, prop)
      }),
      {}
    )
  Object.defineProperties(target, props)
}

global.window = window
global.document = window.document
global.navigator = {
  userAgent: 'node.js'
}

// The search site requires these globals to be defined, in this case they
// are set using environment variables when tests are run.
global.API_URL = process.env.SEARCH_API_URL
global.ENVIRONMENT = process.env.NODE_ENV

copyProps(window, global)

Enzyme.configure({ adapter: new Adapter() })
