import React from 'react';
import userEvent from '@testing-library/user-event';
import { render, fireEvent, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import SignUp from './../auth/signup/signup';
import { invalidSignupEntries, validUser } from './data/inputData';

jest.mock('../serverRequests.js', () => ({
	...jest.requireActual('../serverRequests.js'),
	registerUser: jest.fn().mockReturnValue(false)
}));

it('renders without crashing', () => {
	render(
		<MemoryRouter>
			<SignUp />
		</MemoryRouter>
	);
});

describe('SignUp invalid input test', () => {
	jest.mock('../serverRequests.js', () => ({
		...jest.requireActual('../serverRequests.js'),
		registerUser: jest.fn().mockReturnValue(true)
	}));

	it.each(invalidSignupEntries)(
		'check invalid combination for validity',
		async (signUpEntry) => {
			render(
				<MemoryRouter>
					<SignUp />
				</MemoryRouter>
			);

			const firstNameInput = screen.getByPlaceholderText(/first name/i);
			userEvent.type(firstNameInput, signUpEntry.firstName);
			const lastNameInput = screen.getByPlaceholderText(/last name/i);
			userEvent.type(lastNameInput, signUpEntry.lastName);
			const emailInput = screen.getByPlaceholderText(/example.com/i);
			userEvent.type(emailInput, signUpEntry.email);
			const passwordInput = screen.getByPlaceholderText(/password/i);
			userEvent.type(passwordInput, signUpEntry.password);

			const signUpButton = screen.getByText(/sign up/i);
			expect(signUpButton).toBeEnabled();

			fireEvent.click(signUpButton);
			expect(await screen.findByText(/invalid/i)).not.toBeNull();
		}
	);
});

describe('SignUp valid input test', () => {
	it('valid input success', async () => {
		render(
			<MemoryRouter>
				<SignUp />
			</MemoryRouter>
		);

		const firstNameInput = screen.getByPlaceholderText(/first name/i);
		userEvent.type(firstNameInput, validUser.firstName);

		const submitButton = screen.getByText(/sign up/i);
		expect(submitButton).not.toBeEnabled();

		const lastNameInput = screen.getByPlaceholderText(/last name/i);
		userEvent.type(lastNameInput, validUser.lastName);
		expect(submitButton).not.toBeEnabled();

		const emailInput = screen.getByPlaceholderText(/example.com/i);
		userEvent.type(emailInput, validUser.email);
		expect(submitButton).not.toBeEnabled();

		const passwordInput = screen.getByPlaceholderText(/password/i);
		userEvent.type(passwordInput, validUser.password);
		expect(await screen.findByText(/sign up/i)).toBeEnabled();

		fireEvent.click(submitButton);
		expect(screen.queryByText(/invalid/i)).toBeNull();
	});
});
