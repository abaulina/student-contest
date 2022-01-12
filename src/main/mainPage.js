import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import laptop from '../assets/laptop.png';
import Login from '../auth/login/login';
import useAuth from '../auth/useAuth';
import './mainPage.css';

function Main() {
	const isAuthenticated = useAuth().isAuthenticated;
	const location = useLocation();

	if (isAuthenticated)
		return <Navigate to='/user' replace state={{ from: location.pathname }} />;

	return (
		<div className='d-flex main'>
			<img className='main' data-testid='img' src={laptop} />
			<Login />
		</div>
	);
}

export default Main;
