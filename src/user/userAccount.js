import React from 'react';
import Cookies from 'js-cookie';
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
			<p className='not-found'>
				Nice to see you again, {name} {lastName}
			</p>
		</div>
	);
}

export default UserAccount;
