//import React from 'react';
//import { Navigate, useLocation } from 'react-router-dom';
import PropTypes from 'prop-types';

const PrivateRoute = ({ children }) => {
	//const location = useLocation();

	// if (!isUserLoggedIn)
	// 	return <Navigate to='/login' state={{ from: location }} />;

	return children;
};

PrivateRoute.PropTypes = {
	children: PropTypes.node,
	isUserLoggedIn: PropTypes.bool
};

export default PrivateRoute;
