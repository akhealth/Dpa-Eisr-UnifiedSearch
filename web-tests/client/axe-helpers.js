import axeCore from 'axe-core'
import util from 'util'
import { mount } from 'enzyme'
import React from 'react'
import { Router } from 'react-router-dom'
import { createMemoryHistory } from 'history'

const axeCoreRun = util.promisify(axeCore.run)

// Run aXe tests on a given Enzyme component. Enzyme doesn't
// expose the component on the DOM by default, which aXe requires,
// so here we mount it, run aXe, then unmount it.
const testComponent = component => {
  const wrapper = document.createElement('div')
  document.body.appendChild(wrapper)

  const reactWrapper = mount(component)

  wrapper.innerHTML = ''
  wrapper.appendChild(reactWrapper.getDOMNode())

  return axeCoreRun(reactWrapper.getDOMNode()).then(results => {
    document.body.removeChild(wrapper)
    return results
  })
}

const testLinkedComponent = component => {
  const history = createMemoryHistory()
  component = <Router history={ history }>{component}</Router>

  return testComponent(component)
}

// Format aXe violations for better readability in unit test results.
const report = violations => {
  const reportViolation = v => {
    return `
        Error: ${v.id} - ${v.help}
        Impact: ${v.impact}
        Description: ${v.description}

        ${v.nodes.map(n => n.html).join('\n\t')}`
  }

  return violations.map(v => reportViolation(v)).join('\n') + '\n\n\t'
}

export { testComponent, testLinkedComponent, report }
