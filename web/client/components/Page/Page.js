import React from 'react'
import PropTypes from 'prop-types'

import './Page.scss'

export const Page = ({ className, title, children }) => {
  return (
    <section name={ title } className={ `page ${className}` }>
      {title ? (
        <div className="page-header">
          <h1>{title}</h1>
        </div>
      ) : null}

      <div className="page-content">{children}</div>
    </section>
  )
}

Page.propTypes = {
  className: PropTypes.string,
  title: PropTypes.string,
  children: PropTypes.node
}

export default Page
