import React from 'react';
import laptop from '../assets/laptop.png';
import { Login } from './auth/login.component/login.component';
import './main.component.css';

export function Main() {
	return (
		<div className='d-flex main'>
			<img className='main' src={laptop} />
			<Login />
		</div>
	);
}
