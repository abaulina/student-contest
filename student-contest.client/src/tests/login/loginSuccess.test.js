import React from 'react';
import userEvent from '@testing-library/user-event';
import { render, fireEvent, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import Login from '../../auth/login/login';
import { validLoginEntry } from '../data/inputData';

jest.resetAllMocks();
jest.mock('../../auth/useAuth', () => {
	const originalModule = jest.requireActual('../../auth/useAuth');
	return {
		__esModule: true,
		...originalModule,
		default: () => ({
			accessToken: 'token',
			isAuthenticated: true,
			login: jest.fn,
			logout: jest.fn
		})
	};
});

describe('Login valid input test', () => {
	it('valid input success', async () => {
		render(
			<MemoryRouter>
				<Login />
			</MemoryRouter>
		);

		const emailInput = screen.getByPlaceholderText(/example.com/i);
		userEvent.type(emailInput, validLoginEntry.email);

		const submitButton = screen.getByText(/submit/i);
		expect(submitButton).not.toBeEnabled();

		const passwordInput = screen.getByPlaceholderText(/password/i);
		userEvent.type(passwordInput, validLoginEntry.password);

		expect(await screen.findByText(/submit/i)).toBeEnabled();

		fireEvent.click(submitButton);
		expect(screen.queryByText(/Invalid email or password/i)).toBeNull();
	});
});
