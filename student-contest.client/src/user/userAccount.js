import React from 'react';
import PrivateRoute from './../privateRoute';
import { getUserInfo } from '../serverRequests';
import useAuth from '../auth/useAuth';
import './userAccount.css';

const getLoggedInUserInfo = async () => {
	return await getUserInfo(useAuth().accessToken);
};

async function UserAccount() {
	const loggedInUser = await getLoggedInUserInfo();
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

export function WrappedUserAccount() {
	return (
		<PrivateRoute>
			<UserAccount />
		</PrivateRoute>
	);
}

export default UserAccount;
