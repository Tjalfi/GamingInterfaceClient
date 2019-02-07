package ca.coffeeshopstudio.gaminginterfaceclient.views;

import android.annotation.SuppressLint;
import android.app.AlertDialog;
import android.content.ClipData;
import android.content.Context;
import android.content.DialogInterface;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.ColorDrawable;
import android.graphics.drawable.Drawable;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.support.v4.app.FragmentManager;
import android.util.TypedValue;
import android.view.DragEvent;
import android.view.GestureDetector;
import android.view.MotionEvent;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.SeekBar;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.Toast;

import java.io.IOException;

import ca.coffeeshopstudio.gaminginterfaceclient.R;
import ca.coffeeshopstudio.gaminginterfaceclient.models.Command;
import ca.coffeeshopstudio.gaminginterfaceclient.models.ControlDefaults;
import ca.coffeeshopstudio.gaminginterfaceclient.models.ControlTypes;
import ca.coffeeshopstudio.gaminginterfaceclient.models.GICControl;

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
public class EditActivity extends AbstractGameActivity implements EditTextStyleFragment.EditDialogListener,
        SeekBar.OnSeekBarChangeListener,
        EditImageFragment.EditImageDialogListener,
        EditBackgroundFragment.EditDialogListener {
    private GestureDetector gd;
    private SeekBar seekWidth;
    private SeekBar seekHeight;
    private SeekBar seekFontSize;
    private boolean mode = false;
    private final int minControlSize = 48;
    private final int minFontSize = 4;
    private final int maxFontSize = 256;
    private ControlTypes controlTypes;

    private ControlDefaults defaults;

    protected static final int OPEN_REQUEST_CODE_BACKGROUND = 41;
    protected static final int OPEN_REQUEST_CODE_IMAGE = 42;
    public static final int OPEN_REQUEST_CODE_IMPORT_BUTTON = 43;

    @Override
    protected void onStop() {
        defaults.saveDefaults(this, currentScreen.getScreenId());
        super.onStop();
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        controlTypes = new ControlTypes(getApplicationContext());

        setContentView(R.layout.activity_edit);

        setupFullScreen();
        setupDoubleTap(EditActivity.this);
        setupControls();
        loadScreen();

        defaults = new ControlDefaults(this, currentScreen.getScreenId());

        toggleEditControls(View.GONE, null);
    }

    private void toggleEditControls(int visibility, GICControl control) {
        //always hide on demand OR if in drag/drop mode
        if (visibility == View.GONE || mode) {
            seekFontSize.setVisibility(View.GONE);
            seekHeight.setVisibility(View.GONE);
            seekWidth.setVisibility(View.GONE);
        } else if (currentScreen.getActiveControlId() >= 0) {
            View view = currentScreen.getActiveView();
            //font size only applies for buttons
            if (view instanceof Button) {
                seekFontSize.setVisibility(visibility);
            } else {
                seekFontSize.setVisibility(View.GONE);
            }
            seekHeight.setVisibility(visibility);
            seekWidth.setVisibility(visibility);
            if (visibility == View.VISIBLE && control != null) {
                seekFontSize.setOnSeekBarChangeListener(null);
                seekHeight.setOnSeekBarChangeListener(null);
                seekWidth.setOnSeekBarChangeListener(null);
                seekHeight.setProgress(control.getHeight());
                seekWidth.setProgress(control.getWidth());
                seekFontSize.setProgress(control.getFontSize());
                seekFontSize.setOnSeekBarChangeListener(this);
                seekHeight.setOnSeekBarChangeListener(this);
                seekWidth.setOnSeekBarChangeListener(this);
            }
        }
    }

    private void setupControls() {
        seekWidth = findViewById(R.id.seekWidth);
        seekHeight = findViewById(R.id.seekHeight);
        seekFontSize = findViewById(R.id.seekFontSize);

        findViewById(R.id.topLayout).setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                v.performClick();
                return gd.onTouchEvent(event);
            }
        });
        findViewById(R.id.topLayout).setOnDragListener(new DragDropListener());

        setupToggleSwitch();

        seekWidth.setMax(currentScreen.getMaxControlSize());
        seekHeight.setMax(currentScreen.getMaxControlSize());
        seekFontSize.setMax(maxFontSize);
        seekWidth.setOnSeekBarChangeListener(this);
        seekHeight.setOnSeekBarChangeListener(this);
        seekFontSize.setOnSeekBarChangeListener(this);

        setupButtons();
    }

    private void setupButtons() {
        findViewById(R.id.btnSave).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                currentScreen.saveScreen(findViewById(R.id.topLayout).getBackground());
            }
        });

        findViewById(R.id.btnSettings).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                displayEditBackgroundDialog();
            }
        });
    }

    private void setupToggleSwitch() {
        ((Switch) findViewById(R.id.toggleMode)).setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                mode = b;
                if (mode) {
                    toggleEditControls(View.GONE, null);
                    Toast.makeText(EditActivity.this, R.string.edit_activity_drag_mode, Toast.LENGTH_SHORT).show();
                } else if (currentScreen.getActiveControlId() > -1) {
                    //toggleEditControls(View.VISIBLE);
                    Toast.makeText(EditActivity.this, R.string.edit_activity_detail_edit_mode, Toast.LENGTH_SHORT).show();
                }
            }
        });
    }

    private void setupDoubleTap(final Context context) {
        gd = new GestureDetector(context, new GestureDetector.SimpleOnGestureListener(){
            //here is the method for double tap
            @Override
            public boolean onDoubleTap(MotionEvent e) {
                showControlPopup();
                return true;
            }

            @Override
            public void onLongPress(MotionEvent e) {
                super.onLongPress(e);
            }

            @Override
            public boolean onDoubleTapEvent(MotionEvent e) {
                return true;
            }

            @Override
            public boolean onDown(MotionEvent e) {
                return true;
            }
        });
    }

    private void showControlPopup() {
        AlertDialog.Builder builderSingle = new AlertDialog.Builder(EditActivity.this);

        final ArrayAdapter<String> arrayAdapter = new ArrayAdapter<>(EditActivity.this, android.R.layout.simple_list_item_1, controlTypes.getStringValues());

        builderSingle.setNegativeButton(android.R.string.cancel, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
            }
        });

        builderSingle.setAdapter(arrayAdapter, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                if (controlTypes.getValue(which).equals(getString(R.string.control_type_button)))
                    addButton();
                if (controlTypes.getValue(which).equals(getString(R.string.control_type_text)))
                    addTextView();
                if (controlTypes.getValue(which).equals(getString(R.string.control_type_image)))
                    addImage();
            }
        });
        builderSingle.show();
    }

    private void addImage() {
        GICControl control = initNewControl();

        control.setWidth(defaults.getImageDefaults().getWidth());
        control.setHeight(defaults.getImageDefaults().getHeight());

        buildImage(control);

        updateDisplay(control);
    }

    private void addTextView() {
        //un select any previous button
        GICControl control = initNewControl();

        control.setWidth(defaults.getTextDefaults().getWidth());
        control.setHeight(defaults.getTextDefaults().getHeight());
        control.setFontColor(defaults.getTextDefaults().getFontColor());

        control.setText(getString(R.string.default_control_text));

        buildText(control);

        updateDisplay(control);
    }

    private void addButton() {
        //unselect any previous button
        GICControl control = initNewControl();

        control.setWidth(defaults.getButtonDefaults().getWidth());
        control.setHeight(defaults.getButtonDefaults().getHeight());
        control.setFontSize(defaults.getButtonDefaults().getFontSize());
        control.setFontColor(defaults.getButtonDefaults().getFontColor());
        control.setPrimaryColor(defaults.getButtonDefaults().getPrimaryColor());
        control.setSecondaryColor(defaults.getButtonDefaults().getSecondaryColor());

        control.setText(getString(R.string.default_control_text));

        buildButton(control);

        View view = updateDisplay(control);

        ((Button) view).setTextSize(TypedValue.COMPLEX_UNIT_PX, defaults.getButtonDefaults().getFontSize());
    }

    private GICControl initNewControl() {
        //un select any previous view visually
        unselectedPreviousView();

        return new GICControl();
    }

    private void unselectedPreviousView() {
//        if (activeControl >= 0) {
//            View previous = findViewById(views.get(activeControl).getId());
//            if (previous instanceof Button) {
//                previous.loadBackground(setButtonBackground(primaryColors.get(activeControl), secondaryColors.get(activeControl)));
//            }
//        }
    }

    //called after addControlType style methods
    @SuppressLint("ClickableViewAccessibility")
    private View updateDisplay(GICControl control) {
        View view = currentScreen.getViews().get(currentScreen.getViews().size() - 1);
        view.setOnClickListener(this);
        view.setOnTouchListener(new TouchListener());
        currentScreen.resetActiveControl();

        toggleEditControls(View.GONE, null);

        return view;
    }

    private void displayEditBackgroundDialog() {
        int color = Color.BLACK;
        Drawable background = findViewById(R.id.topLayout).getBackground();
        if (background instanceof ColorDrawable)
            color = ((ColorDrawable) background).getColor();

        int primaryColor = color;
        //int secondaryColor = secondaryColors.get(activeControl);

        FragmentManager fm = getSupportFragmentManager();
        EditBackgroundFragment editBackgroundFragment = EditBackgroundFragment.newInstance(getString(R.string.title_fragment_edit), primaryColor);
        editBackgroundFragment.show(fm, "fragment_edit_background_name");
    }

    private void displayImageEditDialog() {
        FragmentManager fm = getSupportFragmentManager();
        EditImageFragment editFragment = EditImageFragment.newInstance();
        editFragment.show(fm, "fragment_edit_image_name");
    }

    private void displayTextEditDialog() {
        FragmentManager fm = getSupportFragmentManager();
        TextView view = (TextView) currentScreen.getActiveView();

        GICControl control = new GICControl();
        int fontColor = view.getTextColors().getDefaultColor();
        int primaryColor = currentScreen.getActiveControlPrimaryColor();
        int secondaryColor = currentScreen.getActiveControlSecondaryColor();
        String buttonText = (String) view.getText();
        control.setFontColor(fontColor);
        control.setPrimaryColor(primaryColor);
        control.setSecondaryColor(secondaryColor);
        control.setText(buttonText);

        Command commandToSend = ((Command) currentScreen.getActiveView().getTag());
        EditTextStyleFragment editTextDialog = EditTextStyleFragment.newInstance(commandToSend, control, view);
        editTextDialog.show(fm, "fragment_edit_name");
    }

    @Override
    protected void addDragDrop(View view) {
        view.setOnTouchListener(new TouchListener());
    }

    //to fix a case of the onClick firing twice
    private void clickHandler(View view) {
        if (currentScreen.getActiveControlId() == view.getId()) {
            if (view instanceof TextView)
                displayTextEditDialog();
            if (view instanceof ImageView)
                displayImageEditDialog();
        } else {
            unselectedPreviousView();
            currentScreen.setActiveControl(view.getId());

            if (view instanceof Button)
                view.setBackground(setButtonBackground(currentScreen.getActiveControlSecondaryColor(), currentScreen.getActiveControlPrimaryColor()));

            GICControl control = new GICControl();
            control.setWidth(view.getWidth());
            control.setHeight(view.getHeight());
            if (view instanceof Button)
                control.setFontSize((int) ((Button) view).getTextSize());
            toggleEditControls(View.VISIBLE, control);
        }
    }

    @Override
    public void onClick(View view) {
        clickHandler(view);
    }

    @Override
    public void onFinishEditBackgroundDialog(int primaryColor, Uri image) {
        if (image == null)
            findViewById(R.id.topLayout).setBackgroundColor(primaryColor);
        else {
            try {
                Bitmap bitmap = MediaStore.Images.Media.getBitmap(this.getContentResolver(), image);
                Drawable drawable = new BitmapDrawable(getResources(), bitmap);
                findViewById(R.id.topLayout).setBackground(drawable);
            } catch (IOException e) {
                Toast.makeText(this, e.getLocalizedMessage(), Toast.LENGTH_LONG).show();
            }
        }
    }

    private void deleteView() {
        FrameLayout layout = findViewById(R.id.topLayout);
        layout.removeView(currentScreen.getActiveView());
        currentScreen.removeCurrentView();
        toggleEditControls(View.GONE, null);
    }

    @Override
    public void onFinishEditImageDialog(Uri newImageUri) {
        if (newImageUri == null) {
            if (currentScreen.getActiveControlId() >= 0) {
                deleteView();
            }
        } else {
            ImageView image = ((ImageView) currentScreen.getActiveView());
            image.setImageURI(newImageUri);
            resizeImageView(image, seekWidth.getProgress(), seekHeight.getProgress());
        }
    }

    @Override
    public void onFinishEditDialog(Command command, boolean toSave, GICControl control) {
        if (!toSave) {
            if (currentScreen.getActiveControlId() >= 0) {
                deleteView();
            }
        } else {
            currentScreen.setActiveControlPrimaryColor(control.getPrimaryColor());
            currentScreen.setActiveControlSecondaryColor(control.getSecondaryColor());

            View view = currentScreen.getActiveView();

            if (view instanceof Button) {
                view.setBackground(setButtonBackground(control.getPrimaryColor(), control.getSecondaryColor()));
            }

            ((TextView) view).setText(control.getText());
            ((TextView) view).setTextColor(control.getFontColor());
            view.setTag(command);

            defaults.saveControl(view, control.getPrimaryColor(), control.getSecondaryColor());
        }
    }

    private void resizeImageView(int seekBarId, int value) {
        ImageView view = (ImageView) currentScreen.getActiveView();

        int newWidth = view.getWidth();
        int newHeight = view.getHeight();
        switch (seekBarId) {
            case R.id.seekHeight:
                newHeight = value;
                break;
            case R.id.seekWidth:
                newWidth = value;
                break;
        }
        if (newWidth >= minControlSize && newHeight >= minControlSize) {
            resizeImageView(view, newWidth, newHeight);
            defaults.saveControl(view);
        }
    }

    //TextView based controls including buttons
    private void resizeTextView(int seekBarId, int value) {
        TextView view = (TextView) currentScreen.getActiveView();

        int newWidth = view.getWidth();
        int newHeight = view.getHeight();
        int newFont = (int) view.getTextSize();
        switch (seekBarId) {
            case R.id.seekHeight:
                newHeight = value;
                break;
            case R.id.seekWidth:
                newWidth = value;
                break;
            case R.id.seekFontSize:
                newFont = value;
                break;
        }
        if (newWidth >= minControlSize && newHeight >= minControlSize && newFont >= minFontSize) {
            view.setLayoutParams(new FrameLayout.LayoutParams(newWidth, newHeight));
            view.setTextSize(TypedValue.COMPLEX_UNIT_PX, newFont);
            defaults.saveControl(view);
        }
    }

    @Override
    public void onProgressChanged(SeekBar seekBar, int value, boolean b) {
        if (currentScreen.getActiveControlId() >= 0) {
            if (currentScreen.getActiveView() instanceof TextView) {
                resizeTextView(seekBar.getId(), value);
            } else if (currentScreen.getActiveView() instanceof ImageView) {
                resizeImageView(seekBar.getId(), value);
            }
        }
    }

    @Override
    public void onStartTrackingTouch(SeekBar seekBar) {

    }

    @Override
    public void onStopTrackingTouch(SeekBar seekBar) {

    }

    @SuppressWarnings("IntegerDivisionInFloatingPointContext")
    private final class DragDropListener implements View.OnDragListener {
        @Override
        public boolean onDrag(View v, DragEvent event) {
            switch (event.getAction()) {
                case DragEvent.ACTION_DRAG_STARTED:
                    break;
                case DragEvent.ACTION_DRAG_LOCATION:
                    break;
                case DragEvent.ACTION_DRAG_ENTERED:
                    break;
                case DragEvent.ACTION_DRAG_EXITED:
                    break;
                case DragEvent.ACTION_DROP:
                    View view = (View) event.getLocalState();
                    float x = event.getX();
                    float y = event.getY();
                    view.setX(x-(view.getWidth()/2));
                    view.setY(y-(view.getHeight()/2));
                    view.setVisibility(View.VISIBLE);
                    break;
                case DragEvent.ACTION_DRAG_ENDED:
                default:
                    break;
            }
            return true;
        }
    }

    private final class TouchListener implements View.OnTouchListener {
        public boolean onTouch(View view, MotionEvent motionEvent) {
            if (motionEvent.getAction() == MotionEvent.ACTION_DOWN && mode) {
                ClipData data = ClipData.newPlainText("", "");
                View.DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(
                        view);
                view.startDrag(data, shadowBuilder, view, 0);
                view.setVisibility(View.INVISIBLE);
                return true;
            } else if (motionEvent.getAction() == MotionEvent.ACTION_UP) {
                view.performClick();
                //clickHandler(view);
                return true;
            } else {
                return false;
            }
        }
    }
}
