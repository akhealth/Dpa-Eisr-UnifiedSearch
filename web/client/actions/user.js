import superagent from 'superagent'

import state from '-/state'
import logError from './error-handler'

const apiUrl = state.get('config', 'apiUrl')
const user = state.select('user')

export const getUserInfo = async () => {
  try {
    const result = await superagent.get(`${apiUrl}/user`)

    if (result.body.success === true) {
      const userData = result.body.data

      user.set('username', userData.user)
    }
  } catch (error) {
    logError('Error occurred in getUserInfo method of user.js', error)
  }
}
