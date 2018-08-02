import React from 'react'
import { branch } from 'baobab-react/higher-order'
import { Link } from 'react-router-dom'
import PropTypes from 'prop-types'

import { getUserInfo } from '-/actions/user'
import { clearSearch } from '-/actions/search'
import ariesLogo from '-/images/aries-logo.png'
import './Header.scss'

export const Header = ({ showHeader, username }) => {
  if (!showHeader) {
    return false
  }

  if (!username) {
    getUserInfo()
  }

  return (
    <header className="header">
      <div className="header-menu">
        <div className="header-logo">
          <a href={ '/' }>
            <img src={ ariesLogo } alt="ARIES" />
          </a>
        </div>

        <ul className="breadcrumb">
          <li>
            <a href={ '/' }>
              ARIES Home
            </a>
          </li>
          <li>
            <Link to="/" id="search-page-link" onClick={ clearSearch }>
              Search
            </Link>
          </li>
        </ul>

        <div className="user-controls">
          <div className="user-info">
            {username ? <div className="username">{username}</div> : null}
          </div>
          <Link to="/end-session">
            <button className="logout">Logout</button>
          </Link>
        </div>
      </div>
    </header>
  )
}

Header.propTypes = {
  showHeader: PropTypes.bool,
  username: PropTypes.string
}

export default branch(
  {
    showHeader: [ 'showHeader' ],
    username: [ 'user', 'username' ]
  },
  Header
)
