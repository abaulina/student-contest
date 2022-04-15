import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import PropTypes from 'prop-types';
import useAuth from './auth/useAuth';

const ProtectedRoute = ({ children }) => {
	const location = useLocation();
	const isAuthenticated = useAuth().isAuthenticated;

	if (isAuthenticated && !location?.state?.path)
		return <Navigate to='/user' replace />;
	if (isAuthenticated) return <Navigate to={location.state.path} replace />;

	return children;
};

ProtectedRoute.propTypes = {
	children: PropTypes.element
};

export default ProtectedRoute;
