import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import PropTypes from 'prop-types';
import useAuth from './useAuth';

const PrivateRoute = ({ children }) => {
	const location = useLocation();
	const isAuthenticated = useAuth().isAuthenticated;

	if (!isAuthenticated)
		return <Navigate to='/login' replace state={{ path: location.pathname }} />;

	return children;
};

PrivateRoute.propTypes = {
	children: PropTypes.element
};

export default PrivateRoute;
