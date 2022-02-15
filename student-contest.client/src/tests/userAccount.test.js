import React from 'react';
import Cookies from 'js-cookie';
import { render, screen } from '@testing-library/react';
import UserAccount from '../user/userAccount';

let cookieGetSpy = null;
let localStorageGetSpy = null;

beforeEach(() => {
	cookieGetSpy = jest.spyOn(Cookies, 'get').mockReturnValue('test@example.com');
	localStorageGetSpy = jest
		.spyOn(Storage.prototype, 'getItem')
		.mockReturnValue(
			'[{"email":"test@example.com","password":"12345678","firstName":"Test","lastName":"User"}]'
		);
});

afterEach(() => {
	cookieGetSpy.mockRestore();
	localStorageGetSpy.mockRestore();
});

it('renders without crashing', () => {
	render(<UserAccount />);
});

it('renders with correct name', () => {
	render(<UserAccount />);

	expect(
		screen.queryByText(/Nice to see you again, Test User/i)
	).not.toBeNull();
});
