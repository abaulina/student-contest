import React from 'react';
import '@testing-library/jest-dom';
import userEvent from '@testing-library/user-event';
import { render, fireEvent, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import SignUp from './../auth/signup/signup';
import { invalidSignupEntries, validSignupEntry } from './inputData';

let localStorageGetSpy = null;
let localStorageSetSpy = null;

beforeEach(() => {
	localStorageGetSpy = jest
		.spyOn(Storage.prototype, 'getItem')
		.mockReturnValue(
			'[{"email":"test@example.com","password":"12345678","firstName":"Test","lastName":"User"}]'
		);
	localStorageSetSpy = jest.spyOn(Storage.prototype, 'setItem');
});

afterEach(() => {
	localStorageGetSpy.mockRestore();
	localStorageSetSpy.mockRestore();
});

it('renders without crashing', () => {
	render(
		<MemoryRouter>
			<SignUp />
		</MemoryRouter>
	);
});

describe('SignUp input test', () => {
	test.each(invalidSignupEntries)(
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

	it('valid input success', async () => {
		render(
			<MemoryRouter>
				<SignUp />
			</MemoryRouter>
		);

		const firstNameInput = screen.getByPlaceholderText(/first name/i);
		userEvent.type(firstNameInput, validSignupEntry.firstName);

		const submitButton = screen.getByText(/sign up/i);
		expect(submitButton).not.toBeEnabled();

		const lastNameInput = screen.getByPlaceholderText(/last name/i);
		userEvent.type(lastNameInput, validSignupEntry.lastName);
		expect(submitButton).not.toBeEnabled();

		const emailInput = screen.getByPlaceholderText(/example.com/i);
		userEvent.type(emailInput, validSignupEntry.email);
		expect(submitButton).not.toBeEnabled();

		const passwordInput = screen.getByPlaceholderText(/password/i);
		userEvent.type(passwordInput, validSignupEntry.password);
		expect(await screen.findByText(/sign up/i)).toBeEnabled();

		fireEvent.click(submitButton);
		expect(screen.queryByText(/invalid/i)).toBeNull();
	});
});
