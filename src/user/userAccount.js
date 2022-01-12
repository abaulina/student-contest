import React from 'react';
import '@testing-library/jest-dom';
import Cookies from 'js-cookie';
import PrivateRoute from './../privateRoute';
import './userAccount.css';

const getLoggedInUserInfo = () => {
	const userEmail = Cookies.get('userEmail');
	const users = JSON.parse(localStorage.getItem('Users')) || [];
	return users.filter((user) => user.email === userEmail)[0] || null;
};

function UserAccount() {
	const loggedInUser = getLoggedInUserInfo();
	const name = loggedInUser.firstName;
	const lastName = loggedInUser.lastName;

	return (
		<div className='d-flex user-account'>
			<p className='not-found' data-testid='userGreeting'>
				Nice to see you again, {name} {lastName}
			</p>
		</div>
	);
}

export function WrapperUserAccount() {
	return (
		<PrivateRoute>
			<UserAccount />
		</PrivateRoute>
	);
}

export default UserAccount;
