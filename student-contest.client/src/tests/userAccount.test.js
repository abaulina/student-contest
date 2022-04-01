import React from 'react';
import nock from 'nock';
import { render, screen } from '@testing-library/react';
import UserAccount from '../user/userAccount';
import configData from '../config.json';

it('renders without crashing', () => {
	render(<UserAccount />);
});

it('renders with correct name', () => {
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
	// eslint-disable-next-line no-unused-vars
	const scope = nock(configData.SERVER_URL, {
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
