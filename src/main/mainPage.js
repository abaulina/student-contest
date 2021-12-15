import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import PropTypes from 'prop-types';
import laptop from '../assets/laptop.png';
import Login from '../auth/login/login';
import './mainPage.css';

function Main({ isUserLoggedIn, setUserLoggedIn }) {
	const location = useLocation();

	if (isUserLoggedIn)
		return <Navigate to='/user' replace state={{ from: location }} />;

	return (
		<div className='d-flex main'>
			<img className='main' src={laptop} />
			<Login setUserLoggedIn={setUserLoggedIn} />
		</div>
	);
}

Main.propTypes = {
	isUserLoggedIn: PropTypes.bool.isRequired,
	setUserLoggedIn: PropTypes.func.isRequired
};

export default Main;
