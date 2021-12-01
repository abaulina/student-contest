import React, { useState, useRef } from 'react';
import { Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import hidePwdImg from '../../assets/hide-password.svg';
import showPwdImg from '../../assets/show-password.svg';
import * as Yup from 'yup';
import './signup.component.css';

export function SignUp() {
	const localStorage = window.localStorage;
	const [email, setEmail] = useState();
	const [password, setPassword] = useState();
	const [firstName, setFirstName] = useState();
	const [lastName, setLastName] = useState();
	const [isCapsWarningShown, setCapsWarningShown] = useState(false);
	const [isPasswordShown, setPasswordShown] = useState(false);
	const signUpButton = useRef();

	const validationSchema = Yup.object().shape({
		firstname: Yup.string().required('First name is required'),
		lastname: Yup.string().required('Last name is required'),
		email: Yup.string().required('Email is required').email('Email is invalid'),
		password: Yup.string()
			.required('Password is required')
			.min(8, 'Password must be at least 8 characters')
	});

	const {
		register,
		trigger,
		handleSubmit,
		formState: { errors }
	} = useForm({
		resolver: yupResolver(validationSchema)
	});

	const togglePassword = () => {
		setPasswordShown((isPasswordShown) => !isPasswordShown);
	};

	const capsLockWarning = (e) => {
		if (
			e.getModifierState &&
			e.getModifierState('CapsLock') &&
			!isCapsWarningShown
		) {
			toast.error('Caps lock is on', { icon: '❗' });
			setCapsWarningShown((isCapsWarningShown) => !isCapsWarningShown);
		}
	};

	const onPasswordInput = (e) => {
		setPassword(e.target.value);
	};

	const onEmailInput = (e) => {
		setEmail(e.target.value);
	};

	const onFirstNameInput = (e) => {
		setFirstName(e.target.value);
	};

	const onLastNameInput = (e) => {
		setLastName(e.target.value);
	};

	const onPasswordInputBlur = () => {
		toast.remove();
		setCapsWarningShown(false);
	};

	const onSignUpClick = () => {
		const users = JSON.parse(localStorage.getItem('Users')) || [];
		if (users.includes((user) => user.email === email)) {
			trigger('email');
		} else {
			var userData = [
				{ email: email },
				{ password: password },
				{ firstName: firstName },
				{ lastName: lastName }
			];
			users.push(userData);
			localStorage.setItem('Users', JSON.stringify(users));
		}
	};

	// const onEnterPress = (e) => {
	// 	if (e.charCode === 13) {
	// 		e.preventDefault();
	// 		signUpButton.current.click();
	// 	}
	// };

	const FirstNameInput = () => (
		<div className='form-floating'>
			<input
				type='text'
				{...register('firstname')}
				className={`form-control ${errors.firstname ? 'is-invalid' : ''}`}
				id='floatingFirstName'
				onChange={onFirstNameInput}
				//onKeyDown={onEnterPress}
				placeholder='First name'
				autoFocus
			/>
			<label htmlFor='floatingFirstName'>First Name</label>
			<div className='invalid-feedback'>{errors.firstname?.message}</div>
		</div>
	);

	const LastNameInput = () => (
		<div className='form-floating'>
			<input
				type='text'
				{...register('lastname')}
				className={`form-control ${errors.lastname ? 'is-invalid' : ''}`}
				onChange={onLastNameInput}
				//onKeyDown={onEnterPress}
				id='floatingLastName'
				placeholder='Last name'
			/>
			<label htmlFor='floatingLastName'>Last name</label>
			<div className='invalid-feedback'>{errors.lastname?.message}</div>
		</div>
	);

	const EmailInput = () => (
		<div className='form-floating'>
			<input
				type='email'
				{...register('email')}
				className={`form-control ${errors.email ? 'is-invalid' : ''}`}
				onChange={onEmailInput}
				//onKeyDown={onEnterPress}
				id='floatingInput'
				placeholder='name@example.com'
			/>
			<label htmlFor='floatingInput'>Email address</label>
			<div className='invalid-feedback'>{errors.email?.message}</div>
		</div>
	);

	const PasswordInput = () => (
		<div className='form-floating d-flex'>
			<input
				type={isPasswordShown ? 'text' : 'password'}
				{...register('password')}
				className={`form-control ${errors.password ? 'is-invalid' : ''}`}
				id='floatingPassword'
				placeholder='Password'
				onKeyUp={capsLockWarning}
				// onKeyDown={onEnterPress}
				onBlur={onPasswordInputBlur}
				onChange={onPasswordInput}
			/>
			<label htmlFor='floatingPassword'>Password</label>
			<img
				htmlFor='floatingPassword'
				alt=''
				height='24'
				width='24'
				title={isPasswordShown ? 'Hide password' : 'Show password'}
				src={isPasswordShown ? hidePwdImg : showPwdImg}
				onClick={togglePassword}
			/>
			<div className='invalid-feedback'>{errors.password?.message}</div>
		</div>
	);

	const SignUpButton = () => (
		<button
			type='submit'
			className='btn btn-primary btn-block'
			ref={signUpButton}
			disabled={errors ? false : true}
			onClick={handleSubmit(onSignUpClick)}>
			Sign Up
		</button>
	);

	const ForgotPasswordLink = () => (
		<p className='forgot-password text-right'>
			Already registered?
			<Link to='/login'> Sign in</Link>
		</p>
	);

	return (
		<div>
			<h3>Create account</h3>
			<div>
				<FirstNameInput />
				<LastNameInput />
				<EmailInput />
				<PasswordInput />
				<SignUpButton />
				<ForgotPasswordLink />
			</div>
		</div>
	);
}
