import React from 'react';
import nock from 'nock';
import { render, screen } from '@testing-library/react';
import UserAccount from '../user/userAccount';
import configData from '../config.json';

jest.mock('../auth/useAuth', () => {
	const originalModule = jest.requireActual('../auth/useAuth');
	return {
		__esModule: true,
		...originalModule,
		default: () => ({
			isAuthenticated: true,
			accessToken: 'token',
			login: jest.fn,
			logout: jest.fn
		})
	};
});

it('renders without crashing', () => {
	render(<UserAccount />);
});

it('renders with correct name', () => {
	nock(configData.SERVER_URL, {
		reqheaders: {
			authorization: 'Bearer token'
		}
	})
		.get('/')
		.once()
		.reply(200, {
			data: '[{"email":"test@example.com","firstName":"Test","lastName":"User"}]'
		});

	render(<UserAccount />);

	expect(
		screen.queryByText(/Nice to see you again, Test User/i)
	).not.toBeNull();
});
