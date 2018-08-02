/* global describe, it, afterEach */

import '../react-mocha-setup'

import React from 'react'
import { shallow } from 'enzyme'
import { expect } from 'chai'

import { testComponent, report } from '../axe-helpers'
import EndSession from '-/pages/EndSession/EndSession'
import state from '-/state'

describe('EndSession Page', () => {
  afterEach(() => {
    // EndSession uses setTimeout(), which can prevent the tests from
    // exiting unless we explicitly clear the timeout
    window.clearTimeout(state.get('endSessionTimeout'))
  })

  it('should render', () => {
    const component = shallow(<EndSession />)
    expect(component.find('.end-session-page')).to.have.length(1)
  })

  it('should not have accessibility errors', () => {
    return testComponent(<EndSession />).then(({ violations }) => {
      expect(violations.length).to.equal(0, report(violations))
    })
  })
})
