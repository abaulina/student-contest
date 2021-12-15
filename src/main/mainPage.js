import React from 'react';
//import Cookies from 'js-cookie';
//import { Navigate } from 'react-router-dom';
import laptop from '../assets/laptop.png';
import { Login } from '../auth/login/login';
import './mainPage.css';

export function Main() {
	// const isUserLoggedIn = () => (
	//  	if(!Cookies.get('userEmail'))
	// 		return <Navigate to="/home" replace state={state} />
	//   	else
	//   		const userData = JSON.parse(Cookies.get('userEmail'));
	// );

	return (
		<div className='d-flex main'>
			<img className='main' src={laptop} />
			<Login />
		</div>
	);
}
