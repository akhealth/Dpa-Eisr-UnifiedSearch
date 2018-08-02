/* global describe, it, afterEach, Event */

import '../react-mocha-setup'

import React from 'react'
import { shallow } from 'enzyme'
import { expect } from 'chai'

import state from '-/state'
import { IdleMonitor } from '-/components/IdleMonitor/IdleMonitor'

describe('IdleMonitor Component', () => {
  afterEach(() => {
    // IdleMonitor uses setInterval(), which prevents the tests from
    // exiting until we explicitly clear the interval
    window.clearInterval(state.get([ 'idleMonitor', 'timer' ]))
  })

  it('should render', () => {
    const component = shallow(<IdleMonitor enabled={ true } showDebug={ true } />)
    expect(component.find('div')).to.have.length(1)
  })

  it('should NOT render if enabled is false', () => {
    const component = shallow(<IdleMonitor enabled={ false } showDebug={ true } />)
    expect(component.find('div')).to.have.length(0)
  })

  it('should NOT render if showDebug is false', () => {
    const component = shallow(<IdleMonitor enabled={ true } showDebug={ false } />)
    expect(component.find('div')).to.have.length(0)
  })

  it('should NOT create event listeners if a timer already exists', () => {
    const timer = state.select([ 'idleMonitor', 'timer' ])
    timer.set(null)
    shallow(<IdleMonitor enabled={ true } showDebug={ false } timer={ true } />)
    expect(timer.get()).to.equal(null)
  })

  it('should reset the counter on mouse movement', () => {
    shallow(<IdleMonitor enabled={ true } showDebug={ false } />)
    const counter = state.select([ 'idleMonitor', 'counter' ])
    counter.set(100)
    document.dispatchEvent(new Event('mousemove'))
    expect(counter.get()).to.be.lessThan(100)
  })

  it('should reset the counter on key press', () => {
    shallow(<IdleMonitor enabled={ true } showDebug={ false } />)
    const counter = state.select([ 'idleMonitor', 'counter' ])
    counter.set(100)
    document.dispatchEvent(new Event('keydown'))
    expect(counter.get()).to.be.lessThan(100)
  })

  it('should reset the counter on click', () => {
    shallow(<IdleMonitor enabled={ true } showDebug={ false } />)
    const counter = state.select([ 'idleMonitor', 'counter' ])
    counter.set(100)
    document.dispatchEvent(new Event('click'))
    expect(counter.get()).to.be.lessThan(100)
  })
})
