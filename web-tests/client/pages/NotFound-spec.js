/* global describe, it */

import '../react-mocha-setup'

import React from 'react'
import { shallow } from 'enzyme'
import { expect } from 'chai'

import { testComponent, report } from '../axe-helpers'
import NotFound from '-/pages/NotFound/NotFound'

describe('NotFound Page', () => {
  it('should render', () => {
    const component = shallow(<NotFound />)
    expect(component).to.have.length(1)
  })

  it('should not have accessibility errors', () => {
    return testComponent(<NotFound />).then(({ violations }) => {
      expect(violations.length).to.equal(0, report(violations))
    })
  })
})
