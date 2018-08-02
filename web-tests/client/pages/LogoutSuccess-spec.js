/* global describe, it */

import '../react-mocha-setup'

import React from 'react'
import { shallow } from 'enzyme'
import { expect } from 'chai'

import { testComponent, report } from '../axe-helpers'
import LogoutSuccess from '-/pages/LogoutSuccess/LogoutSuccess'

describe('LogoutSuccess Page', () => {
  it('should render', () => {
    const component = shallow(<LogoutSuccess />)
    expect(component.find('.logout-success-page')).to.have.length(1)
  })

  it('should not have accessibility errors', () => {
    return testComponent(<LogoutSuccess />).then(({ violations }) => {
      expect(violations.length).to.equal(0, report(violations))
    })
  })
})
