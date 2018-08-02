/* global describe, it, beforeEach */

import '../react-mocha-setup'

import { expect } from 'chai'
import nock from 'nock'
import Baobab from 'baobab'
import { createMemoryHistory } from 'history'

import state from '-/state'
import * as actions from '-/actions/idle-monitor'

describe('IdleMonitor Actions', () => {
  const initialState = new Baobab(state.deepClone())
  const idleMonitor = state.select('idleMonitor')

  beforeEach(() => {
    state.deepMerge(initialState.deepClone())
  })

  it('should start the counter', () => {
    idleMonitor.set('running', false)
    actions.startCounter()
    expect(idleMonitor.get('running')).to.equal(true)
  })

  it('should stop the counter', () => {
    idleMonitor.set('running', true)
    actions.stopCounter()
    expect(idleMonitor.get('running')).to.equal(false)
  })

  it('should update the counter', () => {
    const initialCounterValue = idleMonitor.get('counter')
    actions.incrementCounter()
    expect(idleMonitor.get('counter')).to.equal(initialCounterValue + 1)
  })

  it('should reset the counter', () => {
    idleMonitor.set('counter', 100)
    actions.resetCounter()
    expect(idleMonitor.get('counter')).to.equal(0)
  })

  it('should stop the counter when value greater than config.timeout', () => {
    const timeout = state.get('config', 'timeout')
    idleMonitor.set('counter', timeout)
    actions.incrementCounter()
    expect(idleMonitor.get('running')).to.equal(false)
  })

  it('should stop counter and clear the timer when counter is greater than config.timeout', () => {
    const timeout = state.get('config', 'timeout')
    idleMonitor.set('counter', timeout)
    state.set('history', createMemoryHistory())
    const timer = window.setInterval(actions.incrementCounter, 1000)
    actions.setTimer(timer)
    actions.incrementCounter()
    expect(idleMonitor.get('running')).to.equal(false)
  })

  it('should refresh the webseal session', async () => {
    idleMonitor.set('sinceRefresh', 1)
    nock(global.API_URL)
      .get('/user/refresh')
      .reply(200, { success: true, data: {} })

    await actions.refreshSession()
    expect(idleMonitor.get('sinceRefresh')).to.equal(0)
  })

  it('should not reset refresh counter if refresh failed', async () => {
    idleMonitor.set('sinceRefresh', 1)
    nock(global.API_URL)
      .get('/user/refresh')
      .reply(200, { success: false, data: {} })

    await actions.refreshSession()
    expect(idleMonitor.get('sinceRefresh')).to.equal(1)
    expect(idleMonitor.get('refreshPending')).to.equal(true)
  })

  it('should refresh the webseal session if counter has exceeded refreshTime', async () => {
    const maxRefreshTime = state.get('config', 'sessionRefresh')
    idleMonitor.set('sinceRefresh', maxRefreshTime + 1)
    nock(global.API_URL)
      .get('/user/refresh')
      .reply(200, { success: true, data: {} })

    await actions.incrementCounter()
    expect(idleMonitor.get('refreshPending')).to.equal(true)
  })

  it('should not refresh the webseal session if refresh is already pending', async () => {
    idleMonitor.set('refreshPending', true)
    nock(global.API_URL)
      .get('/user/refresh')
      .reply(200, { success: true, data: {} })

    await actions.incrementCounter()
    expect(idleMonitor.get('refreshPending')).to.equal(true)
    expect(idleMonitor.get('sinceRefresh')).to.not.equal(0)
  })
})
