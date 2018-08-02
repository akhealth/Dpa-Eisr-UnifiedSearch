/* global describe, it */

import '../react-mocha-setup'

import React from 'react'
import { shallow } from 'enzyme'
import { expect } from 'chai'

import { testComponent, report } from '../axe-helpers'
import { Page } from '-/components/Page/Page'

describe('Page Component', () => {
  it('should render', () => {
    const component = shallow(<Page />)
    expect(component.find('.page-content')).to.have.length(1)
  })

  it('should render with a title', () => {
    const component = shallow(<Page title="Test title" />)
    expect(component.find('.page-header')).to.have.length(1)
  })

  it('should not have accessibility errors', () => {
    return testComponent(<Page />).then(({ violations }) => {
      expect(violations.length).to.equal(0, report(violations))
    })
  })
})
