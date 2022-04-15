import React from 'react';
import laptop from '../assets/laptop.png';
import Login from '../auth/login/login';
import ProtectedRoute from '../protectedRoute';
import './mainPage.css';

function Main() {
	return (
		<div className='d-flex main'>
			<img className='main' data-testid='img' src={laptop} />
			<Login />
		</div>
	);
}

export function WrappedMain() {
	return (
		<ProtectedRoute>
			<Main />
		</ProtectedRoute>
	);
}

export default Main;
