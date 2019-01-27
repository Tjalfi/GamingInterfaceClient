package ca.coffeeshopstudio.gaminginterfaceclient.views;

import android.graphics.Color;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v4.app.DialogFragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.widget.Button;

import ca.coffeeshopstudio.gaminginterfaceclient.R;
import ca.coffeeshopstudio.gaminginterfaceclient.models.Command;
import top.defaults.colorpicker.ColorPickerPopup;

/**
 Copyright [2019] [Terence Doerksen]

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
 */
public class EditBackgroundFragment extends DialogFragment implements View.OnClickListener {
    private int primary;
    private Button btnPrimary;

    public static EditBackgroundFragment newInstance(String title, int primary) {
        EditBackgroundFragment frag = new EditBackgroundFragment();
        Bundle args = new Bundle();
        args.putString("title", title);
        args.putInt("primary", primary);
        frag.setArguments(args);
        return frag;
    }

    // Empty constructor is required for DialogFragment
    // Make sure not to add arguments to the constructor
    // Use `newInstance` instead as shown below
    public EditBackgroundFragment() {
    }

    @Override
    public void onViewCreated(View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);
        // Fetch arguments from bundle and set title
        String title;
        if (getArguments() != null) {
            title = getArguments().getString("title", "Enter Name");
        } else
            title = getString(R.string.default_control_text);
        primary = getArguments().getInt("primary", Color.BLACK);
        getDialog().setTitle(title);
        // Show soft keyboard automatically and request focus to field
        getDialog().getWindow().setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE);
        setupControls(view);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_edit_background, container);
    }

    private void setupControls(View view) {
        btnPrimary = view.findViewById(R.id.btnColor1);
        btnPrimary.setOnClickListener(this);
        btnPrimary.setTextColor(primary);

        view.findViewById(R.id.btnSave).setOnClickListener(this);
    }

    @Override
    public void onClick(View view) {
        EditDialogListener listener = (EditDialogListener) getActivity();
        Command savedCommand = null;
        switch (view.getId()) {
            case R.id.btnSave:
                listener.onFinishEditBackgroundDialog(savedCommand, btnPrimary.getTextColors().getDefaultColor());
                dismiss();
                break;
            case R.id.btnColor1:
                displayColorPicker(view);
                break;
            default:
                break;
        }
    }

    private void displayColorPicker(final View view) {
        int color = ((Button) view).getTextColors().getDefaultColor();
        new ColorPickerPopup.Builder(getActivity())
                .initialColor(color) // Set initial color
                .enableBrightness(true) // Enable brightness slider or not
                .okTitle(getString(R.string.color_picker_title))
                .cancelTitle(getString(android.R.string.cancel))
                .showIndicator(true)
                .showValue(true)
                .build()
                .show(view, new ColorPickerPopup.ColorPickerObserver() {
                    @Override
                    public void onColorPicked(int color) {
                        ((Button) view).setTextColor(color);
                    }
                });
    }


    public interface EditDialogListener {
        void onFinishEditBackgroundDialog(Command command, int primaryColor);
    }

}