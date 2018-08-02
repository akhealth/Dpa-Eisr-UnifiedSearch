/* global describe, it */

import '../react-mocha-setup'

import React from 'react'
import { shallow } from 'enzyme'
import { expect } from 'chai'

import state from '-/state'
import { testLinkedComponent, report } from '../axe-helpers'
import { Header } from '-/components/Header/Header'
import { addUserNock } from '../test-utils'

describe('Header Component', () => {
  it('should render', () => {
    addUserNock()
    const component = shallow(<Header showHeader={ true } />)
    expect(component.find('img')).to.have.length(1)
  })

  it('should render with a username', () => {
    const component = shallow(<Header showHeader={ true } username="Developer" />)
    expect(component.find('.username')).to.have.length(1)
  })

  it('should NOT render if showHeader is false', () => {
    const component = shallow(<Header showHeader={ false } />)
    expect(component.find('img')).to.have.length(0)
  })

  it('should clear search when clicking on the Search link', () => {
    const component = shallow(<Header showHeader={ true } username="Developer" />)
    state.set([ 'searchPage', 'isFirstSearch' ], false)

    component.find('#search-page-link').simulate('click')

    expect(state.get([ 'searchPage', 'isFirstSearch' ])).to.equal(true)
  })

  it('should not have accessibility errors', () => {
    return testLinkedComponent(<Header showHeader={ true } username="Developer" />).then(({ violations }) => {
      expect(violations.length).to.equal(0, report(violations))
    })
  })
})
