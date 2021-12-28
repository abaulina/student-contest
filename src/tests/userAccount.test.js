import React from 'react';
import Cookies from 'js-cookie';
import { unmountComponentAtNode } from 'react-dom';
import { render, act } from '@testing-library/react';
import UserAccount from '../user/userAccount';

let container = null;
let cookieGetSpy = null;
let localStorageGetSpy = null;

beforeEach(() => {
	container = document.createElement('div');
	document.body.appendChild(container);
	cookieGetSpy = jest.spyOn(Cookies, 'get').mockReturnValue('test@example.com');
	localStorageGetSpy = jest
		.spyOn(Storage.prototype, 'getItem')
		.mockReturnValue(
			'[{"email":"test@example.com","password":"12345678","firstName":"Test","lastName":"User"}]'
		);
});

afterEach(() => {
	unmountComponentAtNode(container);
	container.remove();
	container = null;
	cookieGetSpy.mockRestore();
	localStorageGetSpy.mockRestore();
});

it('renders without crashing', () => {
	act(() => {
		render(<UserAccount />, container);
	});
});

it('renders with correct name', () => {
	act(() => {
		render(<UserAccount />, container);
	});

	expect(
		document.querySelector('[data-testid=userGreeting]')
	).toHaveTextContent('Nice to see you again, Test User');
});
