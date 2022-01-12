import React from 'react';
import { Link } from 'react-router-dom';
import './signup.css';

const SignUpSuccess = () => {
	return (
		<div
			className='d-flex align-items-center flex-column not-found'
			data-testid='successMsg'>
			<h3 className='sign-up'>Thanks for signing up </h3>
			<p className='not-found'>
				Your account has been successfully created. Now you can log in
			</p>
			<button className='button default'>
				<Link className='not-found' to={'/login'}>
					Log in
				</Link>
			</button>
		</div>
	);
};

export default SignUpSuccess;
