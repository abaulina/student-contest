import React from 'react';
import '@testing-library/jest-dom';
import { unmountComponentAtNode } from 'react-dom';
import { render, act, fireEvent, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { createMemoryHistory } from 'history';
import SignUp from './../auth/signup/signup';

let container = null;
let localStorageGetSpy = null;
let localStorageSetSpy = null;
let history = null;

beforeEach(() => {
	container = document.createElement('div');
	document.body.appendChild(container);
	localStorageGetSpy = jest
		.spyOn(Storage.prototype, 'getItem')
		.mockReturnValue(
			'[{"email":"test@example.com","password":"12345678","firstName":"Test","lastName":"User"}]'
		);
	localStorageSetSpy = jest.spyOn(Storage.prototype, 'setItem');
	history = createMemoryHistory();
});

afterEach(() => {
	unmountComponentAtNode(container);
	container.remove();
	container = null;
	localStorageGetSpy.mockRestore();
	localStorageSetSpy.mockRestore();
	history = null;
});

it('renders without crashing', () => {
	act(() => {
		render(
			<MemoryRouter history={history}>
				<SignUp />, container
			</MemoryRouter>
		);
	});
});

const invalidSignupEntries = [
	{
		email: 'abc.test.com',
		password: '123',
		firstName: '1.',
		lastName: '1User'
	},
	{
		email: 'example@abc',
		password: 'pass12D',
		firstName: 'Test1',
		lastName: 'User'
	},
	{
		email: '.@yahoo.com',
		password: 'pasW34E',
		firstName: 'Test',
		lastName: 'Use1r'
	},
	{
		email: 'us(E)r@testing.com',
		password: 'Pa$$w0r',
		firstName: 'Test',
		lastName: 'User'
	},
	{
		email: 'u$er@testing.com',
		password: '12345678',
		firstName: 'T_est',
		lastName: 'Use_r'
	},
	{
		email: 'u$er@testing.com',
		password: '12345678',
		firstName: 'Test)',
		lastName: 'User'
	},
	{
		email: 'user.@testing.com',
		password: '12345678',
		firstName: 'Test',
		lastName: 'User?'
	},
	{
		email: 'user@.com',
		password: '12345678',
		firstName: 'Test',
		lastName: 'Use-r'
	},
	{
		email: 'user@example.com',
		password: '123457*',
		firstName: 'Test%',
		lastName: 'User@'
	},
	{
		email: 'user@example.com',
		password: '!onetwo~',
		firstName: '_test',
		lastName: 'Use-r'
	},
	{
		email: 'user@example.com',
		password: 'onetwo',
		firstName: 'Test',
		lastName: 'U$er'
	},
	{
		email: 'user@example.com',
		password: 'onetwothree',
		firstName: 'Test%',
		lastName: 'User.'
	},
	{
		email: 'user@.com',
		password: '12345678',
		firstName: 'Test',
		lastName: 'Use-r'
	},
	{
		email: 'user@example.com',
		password: '123678',
		firstName: 'Test',
		lastName: 'User'
	},
	{
		email: 'user@example.com',
		password: '12345678',
		firstName: '.Test',
		lastName: 'Use-r'
	},
	{
		email: 'user@example.com',
		password: '12345678',
		firstName: 'Test',
		lastName: 'User!'
	}
];

describe('SignUp input test', () => {
	test.each(invalidSignupEntries)(
		'check invalid combination for validity',
		async (signUpEntry) => {
			act(() => {
				render(
					<MemoryRouter history={history}>
						<SignUp />, container
					</MemoryRouter>
				);
			});

			const firstNameInput = screen.queryByPlaceholderText(/first name/i);
			fireEvent.change(firstNameInput, {
				target: { value: signUpEntry.firstName }
			});
			const lastNameInput = screen.queryByPlaceholderText(/last name/i);
			fireEvent.change(lastNameInput, {
				target: { value: signUpEntry.lastName }
			});
			const emailInput = screen.queryByPlaceholderText(/example.com/i);
			fireEvent.change(emailInput, {
				target: { value: signUpEntry.email }
			});
			const passwordInput = screen.queryByPlaceholderText(/password/i);
			fireEvent.change(passwordInput, {
				target: { value: signUpEntry.password }
			});

			const signUpButton = screen.getByText(/sign up/i);
			expect(signUpButton).toBeEnabled();

			fireEvent.click(signUpButton);
			expect(await screen.findByText(/invalid/i)).not.toBeNull();
		}
	);

	it('valid input success', async () => {
		const validEntry = {
			email: 'test@example.com',
			password: '12345678',
			firstName: 'Test',
			lastName: 'User'
		};
		act(() => {
			render(
				<MemoryRouter history={history}>
					<SignUp />, container
				</MemoryRouter>
			);
		});

		const firstNameInput = screen.queryByPlaceholderText(/first name/i);
		fireEvent.change(firstNameInput, {
			target: { value: validEntry.firstName }
		});

		const submitButton = screen.getByText(/sign up/i);
		expect(submitButton).not.toBeEnabled();

		const lastNameInput = screen.queryByPlaceholderText(/last name/i);
		fireEvent.change(lastNameInput, {
			target: { value: validEntry.lastName }
		});
		expect(submitButton).not.toBeEnabled();

		const emailInput = screen.queryByPlaceholderText(/example.com/i);
		fireEvent.change(emailInput, {
			target: { value: validEntry.email }
		});
		expect(submitButton).not.toBeEnabled();

		const passwordInput = screen.queryByPlaceholderText(/password/i);
		fireEvent.change(passwordInput, {
			target: { value: validEntry.password }
		});
		expect(await screen.findByText(/sign up/i)).toBeEnabled();

		fireEvent.click(submitButton);
		expect(screen.queryByText(/invalid/i)).toBeNull();
	});
});
